#pragma once

#include <string>

struct EventInfo
{
	std::string  FirstPhotoID;
	unsigned int PhotoCount;
	time_t       BeginDate;
	time_t       EndDate;
};
