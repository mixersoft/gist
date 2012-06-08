#include "gist.hpp"

#include "json/json.h"

#include <stdlib.h>

#include <fstream>
#include <string>

using namespace std;

int main(int argc, char * argv[])
{
	Json::Value root;
	Json::Reader reader;

	ifstream stream("venice.json");
	if (!reader.parse(stream, root, false))
	{
		cout << "Input JSON data could not be parsed.\n";
		cout << reader.getFormattedErrorMessages();
		return EXIT_FAILURE;
	}

	Json::Value castingCall(root["response"]["castingCall"]);

	Json::Value photos(castingCall["CastingCall"]["Auditions"]["Audition"]);

	for (int i(0), size(photos.size()); i != size; ++i)
	{
		cout << photos[i]["id"].asString() << "\n";
	}

	/*
	Descriptor descriptor;

	int blockCount = 4;
	int a = 8;
	int b = 8;
	int c = 4;
	GetBwDescriptor("test.jpg", blockCount, a, b, c, descriptor);

	ofstream stream("out.txt");
	for (size_t i(0), size(descriptor.size()); i != size; ++i)
		stream << descriptor[i] << '\n';
	*/

	return EXIT_SUCCESS;
}
