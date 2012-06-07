add_executable(
	image-group
	main.cpp
	gist.hpp gist.cpp
	standalone_image.hpp standalone_image.cpp
	)

set_target_properties(
	image-group
	PROPERTIES COMPILE_FLAGS "-ffor-scope -Werror -Wall"
	)

add_definitions(-DUSE_GIST -DSTANDALONE_GIST)

target_link_libraries(image-group libcv.so)
target_link_libraries(image-group libfftw3f.so)
target_link_libraries(image-group libhighgui.so)
