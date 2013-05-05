#pragma once

#include "EventInfo.hpp"
#include "InputData.hpp"

#include <vector>

void Write
	( const InputData              & inputData
	, const std::vector<EventInfo> & events
	, const std::vector<EventInfo> & noise
	,       bool                     prettyPrint
	);

void Read(InputData & inputData);
