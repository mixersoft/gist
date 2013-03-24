#pragma once

#include "EventInfo.hpp"
#include "PhotoInfo.hpp"

#include <vector>

void GroupPhotos
	( const std::vector<PhotoInfo> & photos
	,       std::vector<EventInfo> & events
	,       double                   windowWidth
	,       double                   eventSpacing
	,       int                      iterationCount
	);

void SortPhotos(std::vector<PhotoInfo> & photos);
