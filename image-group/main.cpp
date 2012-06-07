#include "gist.hpp"

#include <fstream>

using namespace std;

int main(int argc, char * argv[])
{
	Descriptor descriptor;

	int blockCount = 4;
	int a = 8;
	int b = 8;
	int c = 4;
	GetBwDescriptor("test.jpg", blockCount, a, b, c, descriptor);

	ofstream stream("out.txt");
	for (size_t i(0), size(descriptor.size()); i != size; ++i)
		stream << descriptor[i] << '\n';
}
