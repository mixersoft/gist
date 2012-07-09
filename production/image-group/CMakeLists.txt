add_executable(
	image-group
	Main.cpp
	Cluster.hpp Cluster.cpp
	PhotoInfo.hpp
	InputData.hpp
	gist.hpp gist.cpp
	standalone_image.hpp standalone_image.cpp
	jsoncpp.cpp json/json.h json/json-forwards.h
	)

set_target_properties(
	image-group
	PROPERTIES COMPILE_FLAGS "-ffor-scope -Werror -Wall -Wextra -ansi"
	)

add_definitions(-DUSE_GIST -DSTANDALONE_GIST -DJSON_IS_AMALGAMATION)

find_package(Boost REQUIRED program_options)
find_package(OpenCV REQUIRED)

include_directories(".")

target_link_libraries(image-group ${Boost_PROGRAM_OPTIONS_LIBRARY})
target_link_libraries(image-group ${OpenCV_LIBS})
target_link_libraries(image-group libfftw3f.so)
