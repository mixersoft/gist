#include "Processor.hpp"

#include <algorithm>
#include <stdexcept>
#include <iostream>

using namespace std;

bool PhotoLessThanByDate
	( const PhotoInfo & lhs
	, const PhotoInfo & rhs
	)
{
	return lhs.DateTaken < rhs.DateTaken;
}

bool PhotoGreaterThanByDate
	( const PhotoInfo & lhs
	, const PhotoInfo & rhs
	)
{
	return lhs.DateTaken > rhs.DateTaken;
}

// Find where each event ends and another begins.
// An event is a set of points no farther than eventSpacing apart.
void Aggregate
	( const vector<double> & data
	,       vector<size_t> & separators
	,       double           eventSpacing
	)
{
	if (data.empty())
		return;
	double prev = data[0];
	for (size_t i(0), size(data.size()); i != size; ++i)
	{
		if (data[i] - prev > eventSpacing)
			separators.push_back(i);
		prev = data[i];
	}
}

// Gather data points around the modes of the kernel density estimate.
void MeanShift
	( const vector<double> & data
	,       vector<double> & modes
	,       double           windowWidth
	,       int              iterationCount
	)
{
	// TODO: O(n²) → O(n)

	const size_t n           (data.size());
	const double widthFactor (1.0 / (windowWidth * windowWidth));

	modes.resize(n);
	copy(data.begin(), data.end(), modes.begin());

	for (int iteration(0); iteration != iterationCount; ++iteration)
	{
		for (size_t i(0); i != n; ++i)
		{
			double xi    (modes[i]);
			double mode  (0.0);
			double total (0.0);
			for (size_t j(0); j != n; ++j)
			{
				// epanechnikov kernel
				double xj(data[j]);
				double weight(1.0 - widthFactor * (xi - xj) * (xi - xj));
				if (weight > 0.0)
				{
					total += weight;
					mode  += xj * weight;
				}
			}
			modes[i] = mode / total;
		}
	}
}

// Gather the necessary event information from photos in every group.
void CreateEvents
	( const vector<PhotoInfo> & photos
	,       vector<size_t>    & separators
	,       vector<EventInfo> & events
	)
{
	if (photos.empty())
		return;

	events.resize(separators.size() + 1);

	size_t firstPhotoIndex (0);
	size_t lastPhotoIndex  (0);
	size_t eventStart      (0);
	size_t eventIndex      (0);
	for (size_t i(1), size(photos.size()); i != size; ++i)
	{
		if (eventIndex != separators.size() && i == separators[eventIndex])
		{
			EventInfo & event(events.at(eventIndex));

			event.FirstPhotoID = photos[firstPhotoIndex].ID;
			event.PhotoCount   = i - eventStart;
			event.BeginDate    = photos[firstPhotoIndex].DateTaken;
			event.EndDate      = photos[lastPhotoIndex].DateTaken;

			firstPhotoIndex = i;
			lastPhotoIndex  = i;
			eventStart      = i;

			++eventIndex;
		}
		if (photos[i].DateTaken < photos[firstPhotoIndex].DateTaken)
			firstPhotoIndex = i;
		if (photos[i].DateTaken > photos[lastPhotoIndex].DateTaken)
			lastPhotoIndex = i;
	}
	if (eventIndex != separators.size())
		throw runtime_error("Event creation indexing error.");

	EventInfo & event(events.at(eventIndex));

	event.FirstPhotoID = photos[firstPhotoIndex].ID;
	event.PhotoCount   = photos.size() - eventStart;
	event.BeginDate    = photos[firstPhotoIndex].DateTaken;
	event.EndDate      = photos[lastPhotoIndex].DateTaken;
}

// Text-mode data visualization for debugging purposes.
string MakeTimelineString(const vector<double> & data, size_t stringLength)
{
	vector<char> chars(stringLength + 1);
	fill (chars.begin(), chars.end() - 1, ' ');

	double min(data[0]);
	double max(data[data.size() - 1]);

	for (size_t i(0), size(data.size()); i != size; ++i)
		chars[(data[i] - min) / (max - min) * (stringLength - 1)] = '*';

	return &chars[0];
}

void DetectEvents
	( const vector<PhotoInfo> & photos
	,       vector<EventInfo> & events
	,       double              windowWidth
	,       double              eventSpacing
	,       int                 iterationCount
	,       bool                verbose
	)
{
	const int    n(photos.size());

	vector<double> data(n);
	vector<double> modes(n);
	for (int i(0); i != n; ++i)
		modes[i] = data[i] = static_cast<double>(photos[i].DateTaken);
	MeanShift(data, modes, windowWidth, iterationCount);

	vector<size_t> separators;
	Aggregate(modes, separators, eventSpacing);

	if (verbose)
	{
		cout << "data:  |" << MakeTimelineString(data,  100) << "|\n";
		cout << "modes: |" << MakeTimelineString(modes, 100) << "|\n";
	}

	CreateEvents(photos, separators, events);
}

void SortPhotos(vector<PhotoInfo> & photos, bool verbose)
{
	bool isSorted(photos.end() == adjacent_find(photos.begin(), photos.end(), &PhotoGreaterThanByDate));
	if (!isSorted)
	{
		if (verbose)
			cout << "The input is unsorted and will be sorted.\n";
		sort(photos.begin(), photos.end(), &PhotoLessThanByDate);
	}
}

