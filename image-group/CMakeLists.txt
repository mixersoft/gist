add_executable(
	image-group
	main.cpp
	gist.hpp gist.cpp
	standalone_image.hpp standalone_image.cpp
	jsoncpp.cpp json/json.h json/json-forwards.h
	)

set_target_properties(
	image-group
	PROPERTIES COMPILE_FLAGS "-ffor-scope -Werror -Wall"
	)

add_definitions(-DUSE_GIST -DSTANDALONE_GIST -DJSON_IS_AMALGAMATION)

include_directories(".")

target_link_libraries(image-group libboost_program_options.so)
target_link_libraries(image-group libcv.so)
target_link_libraries(image-group libfftw3f.so)
target_link_libraries(image-group libhighgui.so)
