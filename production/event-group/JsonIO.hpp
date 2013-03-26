#pragma once

#include "EventInfo.hpp"
#include "InputData.hpp"

#include <vector>

void Write
	( const InputData              & inputData
	, const std::vector<EventInfo> & events
	,       bool                     prettyPrint
	);

void Read(InputData & inputData);
