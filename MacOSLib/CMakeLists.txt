cmake_minimum_required(VERSION 3.10)
project(MacOSLib)

set(CMAKE_C_STANDARD 11)
set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -fobjc-arc")

add_library(MacOSLib SHARED MacOSLib.m)
target_link_libraries(MacOSLib "-framework Cocoa")