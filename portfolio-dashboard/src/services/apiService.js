import { useQuery } from "react-query";

export const fetchOpenPositions = async () => {
  const res = await fetch("https://localhost:7273/api/Trades/open-positions");
  return res.json();
};

export const fetchClosedPositions = async () => {
  const res = await fetch("https://localhost:7273/api/Trades/closed-positions");
  return res.json();
};
