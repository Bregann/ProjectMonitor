import { Alert, Paper } from '@mui/material';
import { GetServerSideProps } from 'next';
import React from 'react';
import { useEffect, useState } from 'react';
import { Container, Row, Col } from 'react-bootstrap';
import { DoGet } from '../Helpers/fetchHelper';
import { ActiveErrors, ErrorData } from '../types/activeErrors';
import { DashboardData } from '../types/dashboardData';

type DashboardProps = {
  dashboardData: DashboardData
  errorData: ActiveErrors
}

const Dashboard = (props: DashboardProps) => {
  const [dashboardData, setDashboardData] = useState<DashboardData>(props.dashboardData);
  const [errorData, setErrorData] = useState<ActiveErrors>(props.errorData);

  useEffect(() =>{
    setInterval(async () => {
      const dashRres = await DoGet('/api/GetData');
      const dashData: DashboardData = await dashRres.json();
      setDashboardData(dashData);

      const errorRes = await DoGet('/api/GetActiveErrors');
      const errorData: ActiveErrors = await errorRes.json();
      setErrorData(errorData);
    }, 3000);
  }, []);

  return (
    <>
      {dashboardData.error && <h1>Error getting data</h1>}
      {errorData.error && !errorData.notFound &&
      <Alert variant="filled" severity="error"> 
        Error getting data from API!
      </Alert>}

      {errorData.errorData &&
        errorData.errorData.map((error: ErrorData) =>
        <Alert variant="filled" severity="error" key={error.errorId}>
          ERROR DETECTED IN {error.projectName} <br/>
          Description: {error.errorDescription} <br/>
          Date started: {error.dateStarted}
        </Alert>
        )}

      {!dashboardData.error && <Container style={{maxWidth: '90%'}}>
        <Row>
          <Col>
            <h3 style={{paddingTop: 5}}>Servers</h3>
          </Col>
          <Col>
            <h3 style={{paddingTop: 5}}>Projects</h3>
          </Col>
        </Row>
        <Row>
          <Col>
            <Row style={{alignItems: 'center', justifyContent: 'center'}}>
            { dashboardData.servers?.map((data, index) => (
              <Col key={index} xs={4} style={{marginLeft: -60, paddingTop: 10}}>
                <Paper elevation={4} sx={{height: 70, width: 200, backgroundColor: '#2c97df', color: 'white'}}>
                  <h6>{ data.serverName }</h6>
                  <p>{ data.systemUptime }</p>
                </Paper>
              </Col>
            ))}
            </Row>
          </Col>
          <Col>
            <Row style={{alignItems: 'center', justifyContent: 'center'}}>
            {dashboardData.projectStatus?.map((data, index) => (
              <Col key={index} xs={3} style={{paddingTop: 10}}>
                {/* If it's running */ data.projectRunning && 
                <Paper elevation={4} sx={{height: 95, width: 200, backgroundColor: '#65ad07', color: 'white'}}>
                  <h6>{data.projectName}</h6>
                  <p>
                    CPU: {data.cpuUsage}% &nbsp; RAM: {data.ramUsage}Mb
                    <br />
                    Uptime: {data.projectUptime}
                  </p>
                </Paper>}
                {/* If it's not */ !data.projectRunning && 
                <Paper elevation={4} sx={{height: 75, width: 200, backgroundColor: '#df2d2d', color: 'white'}}>
                  <h5>{data.projectName}</h5>
                  <h5>PROJECT DOWN</h5>
                </Paper>
                }

              </Col>
            ))}
          </Row>
          </Col>
        </Row>

        <Row>
          <h3 style={{paddingTop: 20, paddingBottom: 5}}>Project Stats</h3>
        </Row>
        <Row>
          <Col xs={3}>
            <Paper sx={{backgroundColor: '#2c97df', color: 'white'}}>
              <h4>Twitch Bot</h4>
              <Row>
                <Col xs={3}>
                  <h6>Users in stream</h6>
                  <p>{dashboardData.twitchBot?.usersInStream}</p>
                </Col>
                <Col xs={3}>
                  <h6>Chat</h6>
                  {dashboardData.twitchBot?.twitchIRCConnectionStatus && 
                  <p style={{backgroundColor: '#65ad07', marginTop: 1}}>
                    Up
                  </p>}
                  {!dashboardData.twitchBot?.twitchIRCConnectionStatus && 
                  <p style={{backgroundColor: '#df2d2d', marginTop: 1}}>
                    Down
                  </p>}
                </Col>
                <Col xs={3}>
                  <h6>PubSub</h6>
                  {dashboardData.twitchBot?.twitchPubSubConnectionStatus && 
                  <p style={{backgroundColor: '#65ad07', marginTop: 1}}>
                    Up
                  </p>}
                  {!dashboardData.twitchBot?.twitchPubSubConnectionStatus && 
                  <p style={{backgroundColor: '#df2d2d', marginTop: 1}}>
                    Down
                  </p>}
                </Col>
                <Col xs={3}>
                  <h6>Last API refresh</h6>
                  <p>{dashboardData.twitchBot?.twitchApiKeyLastRefreshTime}</p>
                </Col>
                <Col xs={3}>
                  <h6>Discord</h6>
                  {dashboardData.twitchBot?.discordConnectionStatus && 
                  <p style={{backgroundColor: '#65ad07', marginTop: 1}}>
                    Up
                  </p>}
                  {!dashboardData.twitchBot?.discordConnectionStatus && 
                  <p style={{backgroundColor: '#df2d2d', marginTop: 1}}>
                    Down
                  </p>}
                </Col>
                <Col xs={3}>
                  <h6>Stream announced</h6>
                  {dashboardData.twitchBot?.streamAnnounced && 
                  <p style={{backgroundColor: '#65ad07', marginTop: 1}}>
                    Yes
                  </p>}
                  {!dashboardData.twitchBot?.streamAnnounced && 
                  <p>
                    No
                  </p>}
                </Col>
                <Col xs={3}>
                  <h6>Stream status</h6>
                  {dashboardData.twitchBot?.streamStatus && 
                  <p style={{backgroundColor: '#65ad07', marginTop: 1}}>
                    Online
                  </p>}
                  {!dashboardData.twitchBot?.streamStatus && 
                  <p>
                    Offline
                  </p>}
                </Col>
                <Col xs={3}>
                  <h6>Stream uptime</h6>
                  <p>{dashboardData.twitchBot?.streamUptime}</p>
                </Col>
                <Col xs={4}>
                  <h6>Daily points enabled</h6>
                  {dashboardData.twitchBot?.streamStatus && 
                  <p style={{backgroundColor: '#65ad07', marginTop: 1}}>
                    Yes
                  </p>}
                  {!dashboardData.twitchBot?.streamStatus && 
                  <p>
                    No
                  </p>}
                </Col>
                <Col xs={4}>
                  <h6>Discord LB update</h6>
                  <p>{dashboardData.twitchBot?.lastDiscordLeaderboardsUpdate}</p>
                </Col>
                <Col xs={4}>
                  <h6>Hours update</h6>
                  <p>{dashboardData.twitchBot?.lastHoursUpdate}</p>
                </Col>
              </Row>

            </Paper>
          </Col>
          <Col xs={3}>
            <Paper sx={{backgroundColor: '#2c97df', color: 'white'}}>
              <h4>RetroAchievements Tracker</h4>
              <Row>
                <Col xs={3}>
                    <h6>Total Games</h6>
                    <p>{dashboardData.retroAchievementsTracker?.totalGames}</p>
                </Col>
                <Col xs={3}>
                    <h6>Total Users</h6>
                    <p>{dashboardData.retroAchievementsTracker?.totalUsers}</p>
                </Col>
                <Col xs={3}>
                    <h6>Last Game Update</h6>
                    <p>{dashboardData.retroAchievementsTracker?.lastGameUpdate}</p>
                </Col>
                <Col xs={3}>
                    <h6>Games Update</h6>
                    <p>{dashboardData.retroAchievementsTracker?.gamesUpdated}</p>
                </Col>
              </Row>
            </Paper>
          </Col>
          <Col xs={3}>
            <Paper sx={{backgroundColor: '#2c97df', color: 'white'}}>
              <h4>FinanceManager</h4>
              <Row>
                <Col>
                    <h6>Last Transaction Update</h6>
                    <p>{dashboardData.financeManager?.lastTransactionUpdate}</p>
                </Col>
                <Col>
                    <h6>Last API Refresh</h6>
                    <p>{dashboardData.financeManager?.lastAPIRefresh}</p>
                </Col>
                <Col>
                    <h6>Last API Status Code</h6>
                    <p>{dashboardData.financeManager?.lastAPIRefreshStatusCode}</p>
                </Col>
              </Row>
            </Paper>
          </Col>
          <Col xs={3}>
            <Paper sx={{backgroundColor: '#2c97df', color: 'white'}}>
              <h4>Cat Bot</h4>
              <Row>
                <Col>
                    <h6>Discord</h6>
                    {dashboardData.catBot?.discordConnectionStatus && 
                    <p style={{backgroundColor: '#65ad07', marginTop: 1}}>
                      Up
                    </p>}
                  {!dashboardData.catBot?.discordConnectionStatus && 
                  <p style={{backgroundColor: '#df2d2d', marginTop: 1}}>
                    Down
                  </p>}
                </Col>
                <Col>
                    <h6>Last Tweet</h6>
                    <p>{dashboardData.catBot?.lastTweet}</p>
                </Col>
                <Col>
                    <h6>Last Discord Post</h6>
                    <p>{dashboardData.catBot?.lastDiscordPost}</p>
                </Col>
              </Row>
            </Paper>
          </Col>
        </Row>
        <footer>
        Last updated at: {dashboardData.lastUpdate}
        </footer>
      </Container> 
      
      }

    </>
  )
}

export const getServerSideProps: GetServerSideProps<DashboardProps> = async (context) => {
    const dashRres = await DoGet('/api/GetData');
    const dashData: DashboardData = await dashRres.json();

      const errorRes = await DoGet('/api/GetActiveErrors');
      const errorData: ActiveErrors = await errorRes.json();

    return {
      props: {
        dashboardData: dashData,
        errorData: errorData
      },
    }
  }

export default Dashboard