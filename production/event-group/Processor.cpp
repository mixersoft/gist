#include "Processor.hpp"

#include <algorithm>
#include <stdexcept>

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

void MeanShift
	( const vector<double> & data
	,       vector<double>   modes
	,       double           windowWidth
	,       int              iterationCount
	)
{
	// TODO: O(n²) → O(n)

	const size_t n           (data.size());
	const double widthFactor (1.0 / (windowWidth * windowWidth));

	modes.resize(n);

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
	size_t eventStart      (0);
	size_t eventIndex      (0);
	for (size_t i(1), size(photos.size()); i != size; ++i)
	{
		if (eventIndex != separators.size() && i == separators[eventIndex])
		{
			events[eventIndex].FirstPhotoID = photos[firstPhotoIndex].ID;
			events[eventIndex].PhotoCount   = i - eventStart;
			firstPhotoIndex = i;
			eventStart      = i;
			++eventIndex;
		}
		if (photos[i].DateTaken < photos[firstPhotoIndex].DateTaken)
			firstPhotoIndex = i;
	}
	if (eventIndex != separators.size())
		throw runtime_error("Event creation indexing error.");
	events[eventIndex].FirstPhotoID = photos[firstPhotoIndex].ID;
	events[eventIndex].PhotoCount   = photos.size() - eventStart;
}

void GroupPhotos
	( const vector<PhotoInfo> & photos
	,       vector<EventInfo> & events
	,       double              windowWidth
	,       double              eventSpacing
	,       int                 iterationCount
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

	CreateEvents(photos, separators, events);
}

void SortPhotos(vector<PhotoInfo> & photos)
{
	bool isSorted(photos.end() == adjacent_find(photos.begin(), photos.end(), &PhotoGreaterThanByDate));
	if (!isSorted)
		sort(photos.begin(), photos.end(), &PhotoLessThanByDate);
}

