import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Table, Button } from 'react-bootstrap';
import ChangeColumn from './ChangeColumn';

const Portfolio = () => {
  const [portfolio, setPortfolio] = useState({
    AAPL: 50,
    THD: 200,
    CYBR: 150,
    ABB: 900
  });
  const [total, setTotal] = useState(0);
  const [rebalanceResult, setRebalanceResult] = useState([]);
  const [desiredPortfolio, setDesiredPortfolio] = useState({
    AAPL: 22,
    THD: 38,
    CYBR: 25,
    ABB: 15
  });

  useEffect(() => {
    setTotal(
        Object.keys(portfolio).reduce((acc, symbol) => {
          return acc + portfolio[symbol] * 100;
        }, 0)
    );
  }, [portfolio]);

  const rebalance = async () => {
    const response = await axios.post('/portfolio/', {
      CurrentPortfolio: portfolio,
      DesiredPortfolio: desiredPortfolio,
      TotalValue: total
    });
    setRebalanceResult(response.data);
  };

  return (
      <div>
        <h2>Current Portfolio</h2>
        <Table striped bordered hover>
          <thead>
          <tr>
            <th>Symbol</th>
            <th>Shares</th>
            <th>Value</th>
          </tr>
          </thead>
          <tbody>
          {Object.keys(portfolio).map(symbol => (
              <tr key={symbol}>
                <td>{symbol}</td>
                <td>{portfolio[symbol]}</td>
                <td>{portfolio[symbol] * 100}</td>
              </tr>
          ))}
          </tbody>
        </Table>
        <p>Total: {total}</p>
        <h2>Desired Portfolio</h2>
        <Table striped bordered hover>
          <thead>
          <tr>
            <th>Symbol</th>
            <th>Percentage</th>
          </tr>
          </thead>
          <tbody>
          {Object.keys(desiredPortfolio).map(symbol => (
              <tr key={symbol}>
                <td>{symbol}</td>
                <td>{desiredPortfolio[symbol]}%</td>
              </tr>
          ))}
          </tbody>
        </Table>
        <button className={"btn btn-primary"} onClick={rebalance}>Rebalance</button>
        {rebalanceResult.length > 0 && (
            <>
              <h2>Rebalance Result</h2>
              <table className={"table table-striped table-bordered table-hover"}>
                <thead>
                <tr>
                  <th>Symbol</th>
                  <th>Shares Bought or Sold</th>
                  <th>New Allocation Percentage</th>
                  <th>Change Since Last Closed</th>
                </tr>
                </thead>
                <tbody>
                {rebalanceResult.map(result => (
                    <tr key={result.symbol}>
                      <td>{result.symbol}</td>
                      <td>{result.sharesBoughtOrSold}</td>
                      <td>{result.newAllocationPercentage}%</td>
                      <ChangeColumn changePercentage={result.changePercentage}/>
                    </tr>
                ))}
                </tbody>
              </table>
            </>
        )}
      </div>
  );
};

export default Portfolio;