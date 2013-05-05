#pragma once

#include "EventInfo.hpp"
#include "PhotoInfo.hpp"

#include <vector>

void DetectEvents
	( const std::vector<PhotoInfo> & photos             // photo input
	,       std::vector<EventInfo> & events             // event output
	,       std::vector<EventInfo> & noise              // noise events
	,       double                   windowWidth        // kernel density estimation window
	,       unsigned int             iterationCount     // number of Mean Shift iterations
	,       double                   coarseEventSpacing // min time between coarse events
	,       double                   fineEventSpacing   // min time between fine events
	,       unsigned int             dayQuota           // min number of events per day to not be noise
	,       bool                     verbose            // output additional information
	);

void SortPhotos
	( std::vector<PhotoInfo> & photos
	, bool                     verbose
	);
