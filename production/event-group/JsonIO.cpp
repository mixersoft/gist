#include "JsonIO.hpp"

#include "json/json.h"

#include <ctime>
#include <iterator>
#include <sstream>
#include <string>
#include <stdexcept>

using namespace std;
using namespace Json;

time_t ParseDateTime(const char * text)
{
	tm time;
	if (NULL == strptime(text, "%Y-%m-%d %T", &time))
	{
		stringstream msg;
		msg << "Failed to parse date '" << text << "'.";
		throw runtime_error(msg.str().c_str());
	}
	return mktime(&time);
}

string PrintDateTime(time_t time)
{
	char buff[20];
	strftime(buff, 20, "%Y-%m-%d %H:%M:%S", localtime(&time));
	return buff;
}

string MakeComment(const string & text)
{
	string result("// ");
	result.append(text);
	return result;
}

void GetEventInfoValue(const EventInfo & event, Value & value, bool prettyPrint)
{
	value["FirstPhotoID"] = event.FirstPhotoID;
	value["PhotoCount"]   = event.PhotoCount;
	value["BeginDate"]    = static_cast<unsigned int>(event.BeginDate);
	value["EndDate"]      = static_cast<unsigned int>(event.EndDate);
	if (prettyPrint)
	{
		value["BeginDate" ].setComment(MakeComment(PrintDateTime(event.BeginDate)), commentAfterOnSameLine);
		value["EndDate"   ].setComment(MakeComment(PrintDateTime(event.EndDate)),   commentAfterOnSameLine);
	}
}

void Read(InputData & inputData)
{
	Value  root;
	Reader reader;

	string doc = string(istream_iterator<char>(cin), istream_iterator<char>());
	if (!reader.parse(doc, root, false))
		throw std::runtime_error(reader.getFormattedErrorMessages());

	Value castingCall (root["response"]["castingCall"]);
	Value photos      (castingCall["CastingCall"]["Auditions"]["Audition"]);

	inputData.ID = castingCall["CastingCall"]["ID"].asUInt();

	inputData.Photos.resize(photos.size());

	for (int i(0), size(photos.size()); i != size; ++i)
	{
		inputData.Photos[i].ID        = photos[i]["id"].asString();
		inputData.Photos[i].DateTaken = ParseDateTime(photos[i]["Photo"]["DateTaken"].asCString());
	}
}

void Write
	( const InputData         & inputData
	, const vector<EventInfo> & events
	, const vector<EventInfo> & noise
	,       bool                prettyPrint
	)
{
	Value result(objectValue);
	result["ID"] = inputData.ID;

	result["Events"] = Value(arrayValue);
	for (int i(0), size(events.size()); i != size; ++i)
	{
		Value event(objectValue);
		GetEventInfoValue(events[i], event, prettyPrint);
		if (events[i].Children.size() > 1)
		{
			event["Children"] = Value(arrayValue);
			for (int j(0), size(events[i].Children.size()); j != size; ++j)
			{
				Value child(objectValue);
				GetEventInfoValue(events[i].Children[j], child, prettyPrint);
				event["Children"].append(child);
			}
		}
		result["Events"].append(event);
	}

	result["Noise"] = Value(arrayValue);
	for (int i(0), size(noise.size()); i != size; ++i)
	{
		Value event(objectValue);
		GetEventInfoValue(noise[i], event, prettyPrint);
		result["Noise"].append(event);
	}

	if (prettyPrint)
		cout << StyledWriter().write(result);
	else
		cout << FastWriter().write(result);
}
