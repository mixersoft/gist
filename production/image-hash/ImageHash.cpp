#include "ImageHash.hpp"

#include <bitset>
#include <stdexcept>
#include <string>
#include <vector>

#include <openssl/md5.h>

#include <opencv/cv.h>
#include <opencv/highgui.h>

using namespace std;

int ExtractHash(const char * path)
{
	// open image
	cv::Mat image(cv::imread(path, CV_LOAD_IMAGE_GRAYSCALE));
	if (image.rows == 0 || image.cols == 0)
		throw std::runtime_error("Image could not be opened.");

	// resize image
	cv::Mat resized;
	const int w(8), h(8);
	cv::resize(image, resized, cv::Size(w, h), 0.0, 0.0, cv::INTER_AREA);

	// to byte array
	const int bpp(2);
	bitset<w * h * bpp> bits;
	int i(0);
	for (int y(0); y != h; ++y)
	for (int x(0); x != w; ++x)
	{
		int value(resized.at<int>(y, x) >> (8 - bpp));
		for (int bit(0); bit != bpp; ++bit)
			bits[i++] = (value & (1 << bit)) != 0;
	}
	string data(bits.to_string());

	// hash the image
	MD5_CTX md5;
	MD5_Init(&md5);
	MD5_Update(&md5, data.c_str(), data.size());

	unsigned char digest[MD5_DIGEST_LENGTH];
	MD5_Final(digest, &md5);

	// get first 32 bits
	return *reinterpret_cast<int*>(digest);
}