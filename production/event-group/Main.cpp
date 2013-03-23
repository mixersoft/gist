#include "InputData.hpp"

#include "json/json.h"

#include <algorithm>
#include <iterator>
#include <stdexcept>
#include <fstream>
#include <string>
#include <vector>

#include <boost/program_options.hpp>

using namespace std;

namespace po = boost::program_options;

void Read(const string & basePath, InputData & inputData)
{
	Json::Value  root;
	Json::Reader reader;

	string doc = string(istream_iterator<char>(cin), istream_iterator<char>());
	if (!reader.parse(doc, root, false))
		throw std::runtime_error(reader.getFormattedErrorMessages());

	Json::Value castingCall (root["response"]["castingCall"]);
	Json::Value photos      (castingCall["CastingCall"]["Auditions"]["Audition"]);

	inputData.ID        = castingCall["CastingCall"]["ID"].asUInt();
	inputData.Timestamp = castingCall["CastingCall"]["Timestamp"].asUInt();

	inputData.Photos.resize(photos.size());

	for (int i(0), size(photos.size()); i != size; ++i)
	{
		inputData.Photos[i].ID   = photos[i]["id"].asString();
		inputData.Photos[i].Path = basePath + photos[i]["Photo"]["Img"]["Src"]["rootSrc"].asString();
	}
}

/*
void Write
	( const InputData          & inputData
	, const vector<PhotoGroup> & groups
	,       bool                 prettyPrint
	)
{
	Json::Value result(Json::objectValue);
	result["ID"]        = inputData.ID;
	result["Timestamp"] = inputData.Timestamp;
	result["Groups"]    = Json::Value(Json::arrayValue);

	for (int i(0), size(groups.size()); i != size; ++i)
	{
		Json::Value group(Json::arrayValue);
		for (int j(0), size(groups[i].size()); j != size; ++j)
			group.append(groups[i][j]->ID);
		result["Groups"].append(group);
	}

	if (prettyPrint)
		cout << Json::StyledWriter().write(result);
	else
		cout << Json::FastWriter().write(result);
}
*/

void Process
	( const string & basePath
	,       bool     //sort
	,       bool     //prettyPrint
	)
{
	InputData inputData;
	Read(basePath, inputData);

	//Write(inputData, groups, prettyPrint);
}

int main(int argc, char * argv[])
try
{
	po::options_description desc("Supported options");
	desc.add_options()
		("help", "display this help message")
		("sort", po::value<bool>()->default_value(false), "set when input is not already sorted by time")
		("base_path", po::value<string>()->default_value("./"), "base path for images")
		("pretty_print", po::value<bool>()->default_value(false), "format output")
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
		( vm["base_path"].as<string>()
		, vm["sort"].as<bool>()
		, vm.count("pretty_print")
		);

	return EXIT_SUCCESS;
}
catch (const std::exception & e)
{
	cerr << e.what() << endl;
	return EXIT_FAILURE;
}
