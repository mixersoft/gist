add_executable(
	event-group
	Main.cpp
	jsoncpp.cpp json/json.h json/json-forwards.h
	)

set_target_properties(
	event-group
	PROPERTIES COMPILE_FLAGS "-ffor-scope -Werror -Wall -Wextra -ansi"
	)

add_definitions(-DJSON_IS_AMALGAMATION)

find_package(Boost REQUIRED program_options)
find_package(OpenCV REQUIRED)

include_directories(".")

target_link_libraries(event-group ${Boost_PROGRAM_OPTIONS_LIBRARY})
target_link_libraries(event-group ${OpenCV_LIBS})
