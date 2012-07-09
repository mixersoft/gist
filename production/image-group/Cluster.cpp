#include "gist.hpp"

#include "cluster.hpp"

#include <cmath>
#include <stdexcept>

using namespace std;

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

void ClusterOrdered
	( const vector<PhotoInfo>           & photos
	,       vector<vector<const PhotoInfo*> > & groups
	,       float                         threshold
	)
{
	Descriptor d1;
	Descriptor d2;

	Descriptor * prevDescriptor(&d1);
	Descriptor * currDescriptor(&d2);

	groups.push_back(PhotoGroup());

	for (int i(0), size(photos.size()); i != size; ++i)
	{
		const PhotoInfo & photo(photos[i]);

		GetBwDescriptor(photo.Path.c_str(), 4, 8, 8, 4, *currDescriptor);

		if (i != 0 && Distance(*currDescriptor, *prevDescriptor) > threshold)
			groups.push_back(PhotoGroup());

		groups.back().push_back(&photo);

		swap(prevDescriptor, currDescriptor);
	}
}
