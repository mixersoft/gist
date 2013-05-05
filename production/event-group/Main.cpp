#include "InputData.hpp"
#include "JsonIO.hpp"
#include "Processor.hpp"

#include <iostream>
#include <stdexcept>
#include <vector>

#include <boost/program_options.hpp>

using namespace std;

namespace po = boost::program_options;

void Process
	( double       scale
	, unsigned int iterationCount
	, unsigned int maxEventSize
	, bool         prettyPrint
	, bool         verbose
	)
{
	const int secondsPerDay(24 * 60 * 60);

	const double windowWidth        (0.5 * scale * secondsPerDay);
	const double fineEventSpacing   (0.5 * secondsPerDay);
	const double coarseEventSpacing (1.0 * secondsPerDay);
	const int    dayQuota           (6);

	InputData inputData;
	Read(inputData);

	if (maxEventSize == 0)
		maxEventSize = inputData.Photos.size();

	SortPhotos(inputData.Photos, verbose);

	std::vector<EventInfo> events;
	DetectEvents
		( inputData.Photos
		, events
		, windowWidth
		, iterationCount
		, fineEventSpacing
		, coarseEventSpacing
		, dayQuota
		, verbose
		);

	Write(inputData, events, prettyPrint);
}

// Main entry point.
// Handles command line arguments and unhandled exceptions.
int main(int argc, char * argv[])
try
{
	double       scale          (1.0);
	unsigned int iterationCount (20);
	unsigned int maxEventSize   (0);
	bool         prettyPrint    (false);
	bool         verbose        (false);

	po::options_description desc("Supported options");
	desc.add_options()
		("help",         "display this help message")
		("scale",        po::value<double>(&scale)->required(),                       "time scale, in days")
		("iterations",   po::value<unsigned int>(&iterationCount)->default_value(20), "number of mean shift iterations")
		("max_event",    po::value<unsigned int>(&maxEventSize),                      "inclusive upper limit on the event size")
		("pretty_print", po::value<bool>(&prettyPrint)->zero_tokens(),                "format output")
		("verbose",      po::value<bool>(&verbose)->zero_tokens(),                    "print additional information")
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

	Process(scale, iterationCount, maxEventSize, prettyPrint, verbose);

	return EXIT_SUCCESS;
}
catch (const std::exception & e)
{
	cerr << e.what() << endl;
	return EXIT_FAILURE;
}
