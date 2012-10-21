#define BOOST_TEST_MODULE image-group test

#include "Cluster.hpp"

#include <map>

#include <boost/test/unit_test.hpp>

using namespace boost;
using namespace std;

struct MockDescriptorLoader
{
	map<string, Descriptor> descriptors;

	void operator () (const string & path, Descriptor & descriptor)
	{
		descriptor = descriptors[path];
	}
};

struct ClusteringFixture
{
	vector<PhotoInfo>  photos;
	vector<PhotoGroup> groups;

	void AddPhoto(const char * path, float value)
	{
		PhotoInfo photo;
		photo.ID   = path;
		photo.Path = path;

		Descriptor descriptor;
		descriptor.push_back(value);

		photos.push_back(photo);
		loader.descriptors[path] = descriptor;
	}

	MockDescriptorLoader loader;
};

// Check the empty input case.
BOOST_FIXTURE_TEST_CASE(UnorderedEmpty, ClusteringFixture)
{
	ClusterUnordered(photos, groups, 2.0, loader);

	BOOST_REQUIRE(groups.empty());
}

// Check the empty input case.
BOOST_FIXTURE_TEST_CASE(OrderedEmpty, ClusteringFixture)
{
	ClusterOrdered(photos, groups, 2.0, loader);

	BOOST_REQUIRE(groups.empty());
}

// Check handling of differences exactly at threshold.
BOOST_FIXTURE_TEST_CASE(UnorderedSplitPoint, ClusteringFixture)
{
	AddPhoto("1", 0.0);
	AddPhoto("2", 1.0);
	AddPhoto("3", 3.0);
	AddPhoto("4", 4.0);

	ClusterUnordered(photos, groups, 2.0, loader);

	BOOST_REQUIRE_EQUAL(groups.size(), 2);
	BOOST_REQUIRE_EQUAL(groups[0].size(), 2);
	BOOST_REQUIRE_EQUAL(groups[0][0]->Path, "1");
	BOOST_REQUIRE_EQUAL(groups[0][1]->Path, "2");
	BOOST_REQUIRE_EQUAL(groups[1].size(), 2);
	BOOST_REQUIRE_EQUAL(groups[1][0]->Path, "3");
	BOOST_REQUIRE_EQUAL(groups[1][1]->Path, "4");
}

// Check handling of differences exactly at threshold.
BOOST_FIXTURE_TEST_CASE(OrderedSplitPoint, ClusteringFixture)
{
	AddPhoto("1", 0.0);
	AddPhoto("2", 1.0);
	AddPhoto("3", 3.0);
	AddPhoto("4", 4.0);

	ClusterOrdered(photos, groups, 2.0, loader);

	BOOST_REQUIRE_EQUAL(groups.size(), 2);
	BOOST_REQUIRE_EQUAL(groups[0].size(), 2);
	BOOST_REQUIRE_EQUAL(groups[0][0]->Path, "1");
	BOOST_REQUIRE_EQUAL(groups[0][1]->Path, "2");
	BOOST_REQUIRE_EQUAL(groups[1].size(), 2);
	BOOST_REQUIRE_EQUAL(groups[1][0]->Path, "3");
	BOOST_REQUIRE_EQUAL(groups[1][1]->Path, "4");
}

// Check for duplicates across groups with unordered clustering.
BOOST_FIXTURE_TEST_CASE(UnorderedCrossGroupDuplicates, ClusteringFixture)
{
	AddPhoto("1", -0.5);
	AddPhoto("2",  0.0);
	AddPhoto("3",  0.5);
	AddPhoto("4",  1.0);

	ClusterUnordered(photos, groups, 0.6, loader);

	BOOST_REQUIRE_EQUAL(groups.size(), 2);
	BOOST_REQUIRE_EQUAL(groups[0].size(), 3);
	BOOST_REQUIRE_EQUAL(groups[0][0]->Path, "1");
	BOOST_REQUIRE_EQUAL(groups[0][1]->Path, "2");
	BOOST_REQUIRE_EQUAL(groups[0][2]->Path, "3");
	BOOST_REQUIRE_EQUAL(groups[1].size(), 1);
	BOOST_REQUIRE_EQUAL(groups[1][0]->Path, "4");
}
