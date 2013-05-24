#include "Processor.hpp"

#include <algorithm>
#include <cmath>
#include <iomanip>
#include <iostream>
#include <set>
#include <stdexcept>
#include <utility>

using namespace std;

// indices of the first and last points in a range
typedef pair<size_t, size_t> Range;

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

void FindEvents
	( const vector<double> & data
	, const set<size_t>    & noise
	,       vector<Range>  & ranges
	,       Range            range
	,       double           spacing
	)
{
	size_t s(range.first), f(range.second);
	while (s != f && noise.end() != noise.find(s))
		++s;
	for (size_t i(s); i != f; ++i)
	{
		if (data[i + 1] - data[i] < spacing)
			continue;
		if (noise.end() != noise.find(i + 1))
			continue;
		ranges.push_back(make_pair(s, i));
		s = i + 1;
	}
	ranges.push_back(make_pair(s, f));
	// trim noise at the end of each event
	for (size_t i(0), size(ranges.size()); i != size; ++i)
	{
		size_t & s(ranges[i].first);
		size_t & f(ranges[i].second);
		while (noise.end() != noise.find(f) && f >= s)
			--f;
	}
}

void GetNoise(const vector<size_t> & separators, vector<Range> & noise, size_t quota)
{
	size_t s(0);
	for (size_t i(0), size(separators.size()); i != size; ++i)
	{
		size_t f(separators[i]);
		if (f - s < quota)
			noise.push_back(make_pair(s, f - 1));
		s = f;
	}
}

void GetIndicesFromRanges(const vector<Range> & ranges, set<size_t> & points)
{
	for (size_t i(0), size(ranges.size()); i != size; ++i)
	{
		size_t s(ranges[i].first), f(ranges[i].second);
		for (size_t j(s); j <= f; ++j)
			points.insert(j);
	}
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
	,       vector<Range>     & ranges
	,       vector<EventInfo> & events
	)
{
	if (photos.empty())
		return;

	events.resize(ranges.size());

	for (size_t i(0), size(ranges.size()); i != size; ++i)
	{
		size_t s(ranges[i].first), f(ranges[i].second);

		EventInfo & event(events.at(i));

		event.FirstPhotoID = photos[s].ID;
		event.PhotoCount   = f - s + 1;
		event.BeginDate    = photos[s].DateTaken;
		event.EndDate      = photos[f].DateTaken;
	}
}

void PrintRanges
	( const vector<double> & data
	, const vector<Range>  & ranges
	,       size_t           stringLength
	, const char           * label
	)
{
	vector<char> chars(stringLength + 1);
	fill(chars.begin(), chars.end() - 1, ' ');

	const double min(data.front());
	const double max(data.back());

	for (int i(0), size(ranges.size()); i != size; ++i)
	{
		size_t s((data[ranges[i].first]  - min) * (stringLength - 1) / (max - min));
		size_t f((data[ranges[i].second] - min) * (stringLength - 1) / (max - min));
		chars.at(s) = '(';
		chars.at(f) = (chars.at(f) == '(') ? 'O' : ')';
	}

	cout << label << "|" << &chars[0] << "|\n";
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
void PrintTimelineString
	( const vector<double> & data
	,       double           min
	,       double           max
	,       size_t           stringLength
	, const char           * label
	)
{
	vector<char> chars(stringLength + 1);
	fill(chars.begin(), chars.end() - 1, ' ');

	for (size_t i(0), size(data.size()); i != size; ++i)
		chars.at((data[i] - min) * (stringLength - 1) / (max - min)) = '*';

	cout << label << "|" << &chars[0] << "|\n";
}

void DetectEvents
	( const vector<PhotoInfo> & photos             // photo input
	,       vector<EventInfo> & events             // event output
	,       vector<EventInfo> & noise              // noise events
	,       double              windowWidth        // kernel density estimation window
	,       unsigned int        iterationCount     // number of Mean Shift iterations
	,       double              coarseEventSpacing // min time between coarse events
	,       double              fineEventSpacing   // min time between fine events
	,       unsigned int        dayQuota           // min number of events per day to not be noise
	,       bool                verbose            // output additional information
	)
{
	const size_t n(photos.size());

	// get raw data points

	vector<double> data(n);
	vector<double> modes(n);
	for (size_t i(0); i != n; ++i)
		modes[i] = data[i] = static_cast<double>(photos[i].DateTaken);

	// find noise

	MeanShift
		( data.begin(),  data.end()
		, modes.begin(), modes.end()
		, windowWidth
		, iterationCount
		);

	vector<size_t> separators;
	Aggregate(modes, separators, 0.1 * windowWidth);

	vector<Range> noiseRanges;
	GetNoise(separators, noiseRanges, dayQuota);

	set<size_t> noiseIndices;
	GetIndicesFromRanges(noiseRanges, noiseIndices);

	CreateEvents(photos, noiseRanges, noise);

	// form events

	vector<Range> ranges;
	FindEvents(data, noiseIndices, ranges, make_pair(0, n - 1), coarseEventSpacing);

	CreateEvents(photos, ranges, events);

	vector<Range>     allSubRanges;
	vector<EventInfo> allChildren;
	for (size_t i(0), size(ranges.size()); i != size; ++i)
	{
		vector<Range> subRanges;
		FindEvents(data, noiseIndices, subRanges, ranges[i], fineEventSpacing);
		CreateEvents(photos, subRanges, events[i].Children);
		if (verbose)
		{
			copy(subRanges.begin(), subRanges.end(), back_inserter(allSubRanges));
			copy(events[i].Children.begin(), events[i].Children.end(), back_inserter(allChildren));
		}
	}

	// display debugging information

	if (verbose)
	{
		const size_t diagramLength(100);

		const double min(data.front());
		const double max(data.back());

		PrintTimelineString(data,  min, max, diagramLength, "data:   ");
		PrintTimelineString(modes, min, max, diagramLength, "modes:  ");

		PrintRanges(data, noiseRanges,  diagramLength, "noise:  ");
		PrintRanges(data, ranges,       diagramLength, "events: ");
		PrintRanges(data, allSubRanges, diagramLength, "        ");

		cout << "\ncoarse event sizes:\n";
		PrintStemAndLeafPlot(events);

		cout << "\nfine event sizes:\n";
		PrintStemAndLeafPlot(allChildren);

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

