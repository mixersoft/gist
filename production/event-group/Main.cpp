#include "InputData.hpp"
#include "JsonIO.hpp"
#include "Processor.hpp"

#include <iostream>
#include <stdexcept>
#include <vector>

#include <boost/program_options.hpp>

using namespace std;

namespace po = boost::program_options;

void Process(int scale, int iterationCount, bool prettyPrint)
{
	const int hoursPerDay      (24);
	const int minutesPerHour   (60);
	const int secondsPerMinute (60);

	const double windowWidth  (0.5 * scale * hoursPerDay * minutesPerHour * secondsPerMinute);
	const double eventSpacing (10.0 * secondsPerMinute);

	InputData inputData;
	Read(inputData);

	SortPhotos(inputData.Photos);

	std::vector<EventInfo> events;
	DetectEvents(inputData.Photos, events, windowWidth, eventSpacing, iterationCount);

	Write(inputData, events, prettyPrint);
}

// Main entry point.
// Handles command line arguments and unhandled exceptions.
int main(int argc, char * argv[])
try
{
	bool prettyPrint    (false);
	int  iterationCount (20);
	int  scale          (1);

	po::options_description desc("Supported options");
	desc.add_options()
		("help",         "display this help message")
		("scale",        po::value<int>(&scale)->required(),                 "time scale, in days")
		("iterations",   po::value<int>(&iterationCount)->default_value(20), "number of mean shift iterations")
		("pretty_print", po::value<bool>(&prettyPrint)->zero_tokens(),       "format output")
		;

	po::variables_map vm;
	try
	{
		po::store(po::parse_command_line(argc, argv, desc), vm);
		if (vm.count("help"))
		{
			cout << desc << '\n';
			return EXIT_SUCCESS;
		}
		po::notify(vm);
	}
	catch (const std::exception & e)
	{
		cerr << e.what() << endl;
		cout << desc << '\n';
		return EXIT_FAILURE;
	}

	Process(scale, iterationCount, prettyPrint);

	return EXIT_SUCCESS;
}
catch (const std::exception & e)
{
	cerr << e.what() << endl;
	return EXIT_FAILURE;
}
