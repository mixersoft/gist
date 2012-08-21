#include "BitArray.hpp"

BitArray::BitArray(size_t size)
	: data((size + 7) / 8) // ceil(size/8)
{
}

void BitArray::Set(size_t index, bool value)
{
	if (value)
		data[index / 8] |= 0x1 << (index & 0x7);
	else
		data[index / 8] &= ~(0x1 << (index & 0x7));
}

size_t BitArray::GetByteCount() const
{
	return data.size();
}

unsigned char * BitArray::GetData()
{
	return &data[0];
}

const unsigned char * BitArray::GetData() const
{
	return &data[0];
}
