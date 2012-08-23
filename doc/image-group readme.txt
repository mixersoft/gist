Description

The app is called image-group. It takes JSON data as input and produces JSON data as output. Images in the input data are assumed to be sorted by time. The output data contains the ID and Timestamp from the original data, together with an array of groups of image identifiers. Grouping is performed by testing the difference between GIST descriptors from consequtive images against the given threshold.


Sample usage

cat sample/venice.json | bin/image-group --base_path sample/ --pretty_print


Supported options

  --help                 display this help message
  --base_path arg (=./)  base path for images
  --threshold arg (=0.5) GIST difference threshold for separating groups
  --pretty_print         format output


Requirements

The app was compiled under 32-bit Debian Linux. It depends on the following libraries:

	* libboost_program_options
	* libboost_test
	* libcv
	* libfftw3f
	* libhighgui
	* libssl

Build instruction for Ubuntu 12.04:
> sudo apt-get install build-essential cmake libboost-program-options-dev libfftw3-dev libcv-dev libhighgui-dev libcvaux-dev libopencv-gpu-dev
> # cd to folder containing 'image-group'
> mkdir release; 
> cd release; cmake ..; make;


