#include "gist.hpp"

#include "cluster.hpp"

#include <cmath>
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

void LoadImage(const string & path, int width, int height, cv::Mat & result)
{
	cv::Mat image(cv::imread(path.c_str(), CV_LOAD_IMAGE_GRAYSCALE));
	if (image.rows == 0 || image.cols == 0)
		throw std::runtime_error("Image could not be opened.");
	cv::resize(image, result, cv::Size(120, 120), 0.0, 0.0, cv::INTER_AREA);
}

void ClusterOrdered
	( const vector<PhotoInfo>  & photos
	,       vector<PhotoGroup> & groups
	,       float                threshold
	)
{
	Descriptor d1, d2;

	Descriptor * prevDescriptor(&d1);
	Descriptor * currDescriptor(&d2);

	groups.push_back(PhotoGroup());

	for (int i(0), size(photos.size()); i != size; ++i)
	{
		const PhotoInfo & photo(photos[i]);

		cv::Mat image;
		LoadImage(photo.Path, 120, 120, image);
		GetBwDescriptor(image, 4, 8, 8, 4, *currDescriptor);

		if (i != 0 && Distance(*currDescriptor, *prevDescriptor) > threshold)
			groups.push_back(PhotoGroup());

		groups.back().push_back(&photo);

		swap(prevDescriptor, currDescriptor);
	}
}
