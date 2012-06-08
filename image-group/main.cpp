#include "gist.hpp"

#include "json/json.h"

#include <algorithm>
#include <cmath>
#include <stdexcept>
#include <fstream>
#include <string>
#include <vector>

#include <boost/program_options.hpp>

using namespace std;

namespace po = boost::program_options;

float Distance(Descriptor d1, Descriptor d2)
{
	if (d1.size() != d2.size())
		throw std::logic_error("Comparing descriptors of different dimensionalities.");
	float sum(0.0f);
	for (int i(0), size(d1.size()); i != size; ++i)
	{
		float delta(d1[i] - d2[i]);
		sum += delta * delta;
	}
	return sqrt(sum);
}

void ProcessData(const string & basePath, float threshold, bool prettyPrint)
{
	Json::Value  root;
	Json::Reader reader;

	ifstream stream("venice.json");
	if (!reader.parse(stream, root, false))
		throw std::runtime_error(reader.getFormattedErrorMessages());

	Descriptor d1;
	Descriptor d2;

	Descriptor * prevDescriptor(&d1);
	Descriptor * currDescriptor(&d2);

	Json::Value castingCall (root["response"]["castingCall"]);
	Json::Value photos      (castingCall["CastingCall"]["Auditions"]["Audition"]);

	Json::Value result(Json::objectValue);
	result["ID"]        = castingCall["CastingCall"]["ID"];
	result["Timestamp"] = castingCall["CastingCall"]["Timestamp"];
	result["Groups"]    = Json::Value(Json::arrayValue);

	Json::Value group(Json::arrayValue);

	for (int i(0), size(photos.size()); i != size; ++i)
	{
		group.append(photos[i]["id"].asString());

		string path(basePath);
		path.append(photos[i]["Photo"]["Img"]["Src"]["rootSrc"].asString());

		GetBwDescriptor(path.c_str(), 4, 8, 8, 4, *currDescriptor);

		if (i != 0 && Distance(*currDescriptor, *prevDescriptor) > threshold)
		{
			result["Groups"].append(group);
			group.clear();
		}

		swap(d1, d2);
	}
	if (!group.empty())
		result["Groups"].append(group);

	if (prettyPrint)
	{
		Json::StyledWriter writer;
		cout << writer.write(result);
	}
	else
	{
		Json::FastWriter writer;
		cout << writer.write(result);
	}
}

int main(int argc, char * argv[])
try
{
	po::options_description desc("Supported options");
	desc.add_options()
		("help", "display this help message")
		("base_path", po::value<string>()->default_value("./"), "base path for images")
		("threshold", po::value<float>()->default_value(0.5f), "GIST difference threshold for separating groups")
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

	ProcessData
		( vm["base_path"].as<string>()
		, vm["threshold"].as<float>()
		, vm.count("pretty_print")
		);

	return EXIT_SUCCESS;
}
catch (const std::exception & e)
{
	cerr << e.what() << endl;
	return EXIT_FAILURE;
}
