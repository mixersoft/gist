#include "InputData.hpp"
#include "Processor.hpp"

#include "json/json.h"

#include <algorithm>
#include <ctime>
#include <iterator>
#include <stdexcept>
#include <fstream>
#include <sstream>
#include <string>
#include <vector>

#include <boost/program_options.hpp>

using namespace std;

namespace po = boost::program_options;

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
		Json::Value event(Json::arrayValue);
		event["FirstPhotoID"] = events[i].FirstPhotoID;
		event["PhotoCount"]   = events[i].PhotoCount;
		result["Events"].append(event);
	}

	if (prettyPrint)
		cout << Json::StyledWriter().write(result);
	else
		cout << Json::FastWriter().write(result);
}

void Process(bool sortPhotos, bool prettyPrint)
{
	InputData inputData;
	Read(inputData);

	if (sortPhotos)
		SortPhotos(inputData.Photos);

	std::vector<EventInfo> events;
	GroupPhotos(inputData.Photos, events);

	Write(inputData, events, prettyPrint);
}

// Main entry point.
// Handles command line arguments and unhandled exceptions.
int main(int argc, char * argv[])
try
{
	po::options_description desc("Supported options");
	desc.add_options()
		("help", "display this help message")
		("scale", po::value<int>()->default_value(1), "time scale, in days")
		("sort", "set when input is not already sorted by time")
		("pretty_print", "format output")
		;

	po::variables_map vm;
	try
	{
		po::store(po::parse_command_line(argc, argv, desc), vm);
		po::notify(vm);
	}
	catch (const std::exception & e)
	{
		cerr << e.what() << endl;
		cout << desc << '\n';
		return EXIT_SUCCESS;
	}
	if (vm.count("help"))
	{
		cout << desc << '\n';
		return EXIT_SUCCESS;
	}

	Process
		( vm.count("sort")
		, vm.count("pretty_print")
		);

	return EXIT_SUCCESS;
}
catch (const std::exception & e)
{
	cerr << e.what() << endl;
	return EXIT_FAILURE;
}
