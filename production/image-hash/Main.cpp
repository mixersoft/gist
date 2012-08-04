#include "ImageHash.hpp"

#include <stdlib.h>

#include <iostream>

using namespace std;

void PrintHelp()
{
	cout << "syntax: image-hash <image path>\n";
}

int main(int argc, char * argv[])
try
{
	if (argc == 2)
		cout << ExtractHash(argv[1]) << '\n';
	else
		PrintHelp();
	return EXIT_SUCCESS;
}
catch (const std::exception & e)
{
	cerr << e.what() << endl;
	return EXIT_FAILURE;
}
