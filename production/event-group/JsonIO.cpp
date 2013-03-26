#include "JsonIO.hpp"

#include "json/json.h"

#include <ctime>
#include <iterator>
#include <sstream>
#include <stdexcept>

using namespace std;

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

void Read(InputData & inputData)
{
	Json::Value  root;
	Json::Reader reader;

	string doc = string(istream_iterator<char>(cin), istream_iterator<char>());
	if (!reader.parse(doc, root, false))
		throw std::runtime_error(reader.getFormattedErrorMessages());

	Json::Value castingCall (root["response"]["castingCall"]);
	Json::Value photos      (castingCall["CastingCall"]["Auditions"]["Audition"]);

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
	,       bool                prettyPrint
	)
{
	Json::Value result(Json::objectValue);
	result["ID"]     = inputData.ID;
	result["Events"] = Json::Value(Json::arrayValue);

	for (int i(0), size(events.size()); i != size; ++i)
	{
		Json::Value event(Json::objectValue);
		event["FirstPhotoID"] = events[i].FirstPhotoID;
		event["PhotoCount"]   = events[i].PhotoCount;
		result["Events"].append(event);
	}

	if (prettyPrint)
		cout << Json::StyledWriter().write(result);
	else
		cout << Json::FastWriter().write(result);
}
