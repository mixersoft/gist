#pragma once

#include <string>
#include <vector>

struct EventInfo
{
	std::string  FirstPhotoID;
	unsigned int PhotoCount;
	time_t       BeginDate;
	time_t       EndDate;

	std::vector<EventInfo> Children;
};
