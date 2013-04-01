#pragma once

#include "EventInfo.hpp"
#include "PhotoInfo.hpp"

#include <vector>

void DetectEvents
	( const std::vector<PhotoInfo> & photos
	,       std::vector<EventInfo> & events
	,       double                   windowWidth
	,       double                   eventSpacing
	,       unsigned int             maxEventSize
	,       unsigned int             iterationCount
	,       bool                     verbose
	);

void SortPhotos
	( std::vector<PhotoInfo> & photos
	, bool                     verbose
	);
