add_executable(
	image-group-test
	Main.cpp
	TestCluster.cpp
	../image-group/Cluster.cpp
	../image-group/Cluster.hpp
	../image-group/InputData.hpp
	../image-group/PhotoInfo.hpp
	../image-group/gist.cpp
	../image-group/gist.hpp
	../image-group/json/json-forwards.h
	../image-group/json/json.h
	../image-group/jsoncpp.cpp
	../image-group/standalone_image.cpp
	../image-group/standalone_image.hpp
	)

set_target_properties(
	image-group-test
	PROPERTIES COMPILE_FLAGS "-ffor-scope -Werror -Wall -Wextra -ansi"
	)

add_definitions(
	-DBOOST_TEST_DYN_LINK
	-DJSON_IS_AMALGAMATION
	-DSTANDALONE_GIST
	-DUSE_GIST
	)

find_package(Boost COMPONENTS unit_test_framework program_options REQUIRED)
find_package(OpenCV REQUIRED)

include_directories("." "../image-group")

target_link_libraries(image-group-test ${Boost_UNIT_TEST_FRAMEWORK_LIBRARY})
target_link_libraries(image-group-test ${OpenCV_LIBS})
target_link_libraries(image-group-test libfftw3f.so)
