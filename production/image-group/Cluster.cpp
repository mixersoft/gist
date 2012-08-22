#include "Cluster.hpp"

#include <cmath>
#include <set>
#include <string>
#include <stdexcept>

#include <opencv/cv.h>
#include <opencv/highgui.h>

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

void LoadDescriptor(const string & path, Descriptor & descriptor)
{
	cv::Mat image(cv::imread(path.c_str(), CV_LOAD_IMAGE_GRAYSCALE));
	if (image.rows == 0 || image.cols == 0)
		throw std::runtime_error("Image could not be opened.");
	cv::Mat resized;
	cv::resize(image, resized, cv::Size(120, 120), 0.0, 0.0, cv::INTER_AREA);
	GetBwDescriptor(resized, 4, 8, 8, 4, descriptor);
}

void ClusterOrdered
	( const vector<PhotoInfo>  & photos
	,       vector<PhotoGroup> & groups
	,       float                threshold
	,       DescriptorLoader     LoadDescriptor
	)
{
	Descriptor d1, d2;

	Descriptor * prevDescriptor(&d1);
	Descriptor * currDescriptor(&d2);

	if (!photos.empty())
		groups.push_back(PhotoGroup());

	for (int i(0), size(photos.size()); i != size; ++i)
	{
		const PhotoInfo & photo(photos[i]);

		LoadDescriptor(photo.Path, *currDescriptor);

		if (i != 0 && Distance(*currDescriptor, *prevDescriptor) >= threshold)
			groups.push_back(PhotoGroup());

		groups.back().push_back(&photo);

		swap(prevDescriptor, currDescriptor);
	}
}

// QT clustering algorithm
void ClusterUnordered
	( const vector<PhotoInfo>  & photos
	,       vector<PhotoGroup> & groups
	,       float                threshold
	,       DescriptorLoader     LoadDescriptor
	)
{
	const int n(photos.size());

	// get the descriptors

	vector<Descriptor> descriptors(n);
	for (int i(0); i != n; ++i)
		LoadDescriptor(photos[i].Path, descriptors[i]);

	// cache all difference comparisons

	vector<bool> withinThreshold(n * n);
	for (int i(0); i != n; ++i)
	for (int j(0); j != n; ++j)
	{
		float distance(Distance(descriptors[i], descriptors[j]));
		withinThreshold[i * n + j] = distance < threshold;
	}

	// list all indices

	set<int> indices;
	for (int i(0); i != n; ++i)
		indices.insert(i);

	while (!indices.empty())
	{
		// find the largest cluster

		int   maxIndex (0);
		float maxCount (0);
		for (set<int>::const_iterator i(indices.begin()), end(indices.end()); i != end; ++i)
		{
			int count(0);
			for (int j(0); j != n; ++j)
			{
				if (withinThreshold[*i * n + j])
					++count;
			}
			if (count > maxCount)
			{
				maxIndex = *i;
				maxCount = count;
			}
		}

		// save the cluster and its member's indices from further consideration

		groups.push_back(PhotoGroup());
		for (int j(0); j != n; ++j)
		{
			if (withinThreshold[maxIndex * n + j])
			{
				groups.back().push_back(&photos[j]);
				indices.erase(j);
			}
		}
	}
}
