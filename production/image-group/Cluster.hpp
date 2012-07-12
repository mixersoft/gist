#pragma once

#include "PhotoInfo.hpp"

#include "gist.hpp"

#include <boost/function.hpp>

#include <string>
#include <vector>

typedef std::vector<const PhotoInfo*> PhotoGroup;

typedef boost::function<void(const std::string & path, Descriptor & descriptor)> DescriptorLoader;

void LoadDescriptor(const std::string & path, Descriptor & descriptor);

void ClusterOrdered
	( const std::vector<PhotoInfo>  & photos
	,       std::vector<PhotoGroup> & groups
	,       float                     threshold
	,       DescriptorLoader          LoadDescriptor = LoadDescriptor
	);

void ClusterUnordered
	( const std::vector<PhotoInfo>  & photos
	,       std::vector<PhotoGroup> & groups
	,       float                     threshold
	,       DescriptorLoader          LoadDescriptor = LoadDescriptor
	);
