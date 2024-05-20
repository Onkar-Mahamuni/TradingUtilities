import { useQuery } from "@tanstack/react-query";
import React from "react";
import {
  fetchClosedPositions,
  fetchOpenPositions,
} from "../services/apiService";

function Home() {
  const { data: openPositions, isLoading: isOpenPositionsLoading } = useQuery({
    queryKey: ["openPositions"],
    queryFn: fetchOpenPositions,
  });

  const { data: closedPositions, isLoading: isClosedPositionsLoading } =
    useQuery({
      queryKey: ["closedPositions"],
      queryFn: fetchClosedPositions,
    });

  console.log(openPositions);
  console.log(closedPositions);

  if (isOpenPositionsLoading || isClosedPositionsLoading)
    return <div>Loading</div>;

  return <div>Home</div>;
}

export default Home;
