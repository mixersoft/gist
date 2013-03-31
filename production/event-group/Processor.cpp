#include "Processor.hpp"

#include <algorithm>
#include <cmath>
#include <iomanip>
#include <iostream>
#include <stdexcept>
#include <utility>

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
	separators.push_back(data.size());
}

// Gather data points around the modes of the kernel density estimate.
void MeanShift
	( vector<double>::const_iterator dataBegin
	, vector<double>::const_iterator dataEnd
	, vector<double>::iterator       modesBegin
	, vector<double>::iterator       modesEnd
	, double                         windowWidth
	, int                            iterationCount
	)
{
	// TODO: O(n²) → O(n)

	const double widthFactor (1.0 / (windowWidth * windowWidth));

	for (int iteration(0); iteration != iterationCount; ++iteration)
	{
		for (vector<double>::iterator i(modesBegin); i != modesEnd; ++i)
		{
			double xi    (*i);
			double mode  (0.0);
			double total (0.0);
			for (vector<double>::const_iterator j(dataBegin); j != dataEnd; ++j)
			{
				// epanechnikov kernel
				double xj(*j);
				double weight(1.0 - widthFactor * (xi - xj) * (xi - xj));
				if (weight > 0.0)
				{
					total += weight;
					mode  += xj * weight;
				}
			}
			*i = mode / total;
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

	events.resize(separators.size());

	size_t firstPhotoIndex (0);
	size_t lastPhotoIndex  (0);
	size_t eventStart      (0);
	size_t photoIndex      (0);
	for (size_t i(0), size(separators.size()); i != size; ++i)
	{
		for (; photoIndex != separators[i]; ++photoIndex)
		{
			if (photos[photoIndex].DateTaken < photos[firstPhotoIndex].DateTaken)
				firstPhotoIndex = photoIndex;
			if (photos[photoIndex].DateTaken > photos[lastPhotoIndex].DateTaken)
				lastPhotoIndex = photoIndex;
		}

		EventInfo & event(events.at(i));

		event.FirstPhotoID = photos[firstPhotoIndex].ID;
		event.PhotoCount   = separators[i] - eventStart;
		event.BeginDate    = photos[firstPhotoIndex].DateTaken;
		event.EndDate      = photos[lastPhotoIndex].DateTaken;

		firstPhotoIndex = lastPhotoIndex = eventStart = separators[i];
	}
}

void PrintStemAndLeafPlot(const vector<EventInfo> & events)
{
	if (events.empty())
		return;

	vector<unsigned int> data(events.size());
	for (size_t i(0), size(events.size()); i != size; ++i)
		data[i] = events[i].PhotoCount;
	sort(data.begin(), data.end());

	int stemWidth(0);
	for (size_t i(0), size(data.size()); i != size; ++i)
	{
		int stem(data[i] / 10);
		if (stem != 0)
			stemWidth = max(stemWidth, (int)log10((double)stem));
	}
	stemWidth += 1;

	for (size_t i(0), size(data.size()); i != size; ++i)
	{
		if (i == 0 || data[i] / 10 != data[i - 1] / 10)
			cout << setw(stemWidth) << (data[i] / 10) << " |";

		cout << " " << (data[i] % 10);

		if (i == size - 1 || data[i] / 10 != data[i + 1] / 10)
			cout << "\n";
	}
}

// Text-mode data visualization for debugging purposes.
void PrintTimelineString(const vector<double> & data, size_t stringLength, const char * label)
{
	vector<char> chars(stringLength + 1);
	fill(chars.begin(), chars.end() - 1, ' ');

	double min(data[0]);
	double max(data[data.size() - 1]);

	for (size_t i(0), size(data.size()); i != size; ++i)
		chars[(data[i] - min) / (max - min) * (stringLength - 1)] = '*';

	cout << label << "|" << &chars[0] << "|\n";
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
	const int n(photos.size());

	vector<double> data(n);
	vector<double> modes(n);
	for (int i(0); i != n; ++i)
		modes[i] = data[i] = static_cast<double>(photos[i].DateTaken);

	vector<size_t> separators;

	MeanShift
		( data.begin(),  data.end()
		, modes.begin(), modes.end()
		, windowWidth
		, iterationCount
		);

	separators.clear();
	Aggregate(modes, separators, eventSpacing);

	CreateEvents(photos, separators, events);

	if (verbose)
	{
		PrintTimelineString(data,  100, "data:  ");
		PrintTimelineString(modes, 100, "modes: ");

		cout << "\nevent sizes:\n";
		PrintStemAndLeafPlot(events);

		cout << endl;
	}
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

