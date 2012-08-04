add_executable(
	image-hash
	Main.cpp
	ImageHash.hpp ImageHash.cpp
	)

set_target_properties(
	image-hash
	PROPERTIES COMPILE_FLAGS "-ffor-scope -Werror -Wall -Wextra -ansi"
	)

find_package(Boost REQUIRED program_options)
find_package(OpenCV REQUIRED)

include_directories(".")

target_link_libraries(image-hash ${Boost_PROGRAM_OPTIONS_LIBRARY})
target_link_libraries(image-hash ${OpenCV_LIBS})
target_link_libraries(image-hash ssl)
