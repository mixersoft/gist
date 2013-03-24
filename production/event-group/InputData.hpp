#pragma once

#include "PhotoInfo.hpp"

#include <vector>

struct InputData
{
	unsigned int ID;
	std::vector<PhotoInfo> Photos;
};
