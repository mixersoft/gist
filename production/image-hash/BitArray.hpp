#pragma once

#include <vector>

// A bit array class supporting raw data access.
class BitArray
{
private:

	std::vector<unsigned char> data;

public:

	BitArray(std::size_t size);

	void Set(std::size_t i, bool value);

	std::size_t GetByteCount() const;

	unsigned char * GetData();

	const unsigned char * GetData() const;
};
