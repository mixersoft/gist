#include "InputData.hpp"
#include "JsonIO.hpp"
#include "Processor.hpp"

#include <iostream>
#include <stdexcept>
#include <vector>

#include <boost/program_options.hpp>

using namespace std;

namespace po = boost::program_options;

void Process(double scale, int iterationCount, bool prettyPrint, bool verbose)
{
	const int hoursPerDay      (24);
	const int minutesPerHour   (60);
	const int secondsPerMinute (60);
	const double eventSpacingToWindowWidthRatio(0.05);

	const double windowWidth  (0.5 * scale * hoursPerDay * minutesPerHour * secondsPerMinute);
	const double eventSpacing (eventSpacingToWindowWidthRatio * windowWidth);

	InputData inputData;
	Read(inputData);

	SortPhotos(inputData.Photos, verbose);

	std::vector<EventInfo> events;
	DetectEvents(inputData.Photos, events, windowWidth, eventSpacing, iterationCount, verbose);

	Write(inputData, events, prettyPrint);
}

// Main entry point.
// Handles command line arguments and unhandled exceptions.
int main(int argc, char * argv[])
try
{
	double scale          (1.0);
	int    iterationCount (20);
	bool   prettyPrint    (false);
	bool   verbose        (false);

	po::options_description desc("Supported options");
	desc.add_options()
		("help",         "display this help message")
		("scale",        po::value<double>(&scale)->required(),              "time scale, in days")
		("iterations",   po::value<int>(&iterationCount)->default_value(20), "number of mean shift iterations")
		("pretty_print", po::value<bool>(&prettyPrint)->zero_tokens(),       "format output")
		("verbose",      po::value<bool>(&verbose)->zero_tokens(),           "print additional information")
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

	Process(scale, iterationCount, prettyPrint, verbose);

	return EXIT_SUCCESS;
}
catch (const std::exception & e)
{
	cerr << e.what() << endl;
	return EXIT_FAILURE;
}
