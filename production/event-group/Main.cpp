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
	( double       coarseEventSpacing
	, double       fineEventSpacing
	, unsigned int dayQuota
	, unsigned int iterationCount
	, bool         prettyPrint
	, bool         verbose
	)
{
	const double secondsPerDay (24 * 60 * 60);
	const double windowWidth   (0.5);

	InputData inputData;
	Read(inputData);

	SortPhotos(inputData.Photos, verbose);

	std::vector<EventInfo> events;
	std::vector<EventInfo> noise;
	DetectEvents
		( inputData.Photos
		, events
		, noise
		, windowWidth * secondsPerDay
		, iterationCount
		, coarseEventSpacing * secondsPerDay
		, fineEventSpacing * secondsPerDay
		, dayQuota
		, verbose
		);

	Write(inputData, events, noise, prettyPrint);
}

// Main entry point.
// Handles command line arguments and unhandled exceptions.
int main(int argc, char * argv[])
try
{
	double       coarseEventSpacing (1.0);
	double       fineEventSpacing   (0.5);
	unsigned int iterationCount     (20);
	unsigned int dayQuota           (6);
	bool         prettyPrint        (false);
	bool         verbose            (false);

	po::options_description desc("Supported options");
	desc.add_options()
		("help",           "display this help message")
		("iterations",     po::value<unsigned int>(&iterationCount)->default_value(iterationCount),   "number of mean shift iterations")
		("coarse_spacing", po::value<double>(&coarseEventSpacing)->default_value(coarseEventSpacing), "coarse event spacing, in days")
		("fine_spacing",   po::value<double>(&fineEventSpacing)->default_value(fineEventSpacing),     "fine event spacing, in days")
		("day quota",      po::value<unsigned int>(&dayQuota)->default_value(dayQuota),               "days with fewer photos count as noise")
		("pretty_print",   po::value<bool>(&prettyPrint)->zero_tokens(),                              "format output")
		("verbose",        po::value<bool>(&verbose)->zero_tokens(),                                  "print additional information")
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

	Process(coarseEventSpacing, fineEventSpacing, dayQuota, iterationCount, prettyPrint, verbose);

	return EXIT_SUCCESS;
}
catch (const std::exception & e)
{
	cerr << e.what() << endl;
	return EXIT_FAILURE;
}
