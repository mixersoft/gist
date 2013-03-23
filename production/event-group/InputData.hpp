#pragma once

#include "PhotoInfo.hpp"

#include <vector>

struct InputData
{
	unsigned int ID;
	unsigned int Timestamp;

	std::vector<PhotoInfo> Photos;
};
