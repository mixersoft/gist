#pragma once

#include "PhotoInfo.hpp"

#include <string>
#include <vector>

typedef std::vector<const PhotoInfo*> PhotoGroup;

void ClusterOrdered
	( const std::vector<PhotoInfo>  & photos
	,       std::vector<PhotoGroup> & groups
	,       float                     threshold
	);
