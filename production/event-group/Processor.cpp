#include "Processor.hpp"

#include <algorithm>

using namespace std;

bool ComparePhotosByDate
	( const PhotoInfo & lhs
	, const PhotoInfo & rhs
	)
{
	return lhs.DateTaken < rhs.DateTaken;
}

void GroupPhotos
	( const vector<PhotoInfo> & //photos
	,       vector<EventInfo> & //events
	)
{
}

void SortPhotos(vector<PhotoInfo> & photos)
{
	sort(photos.begin(), photos.end(), &ComparePhotosByDate);
}

