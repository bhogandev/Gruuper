import React, { useState, useEffect } from 'react';
import { Button, Col, Row } from 'react-bootstrap';
import {getFriendList} from '../middleware/BBVAPI';

export default function FriendsView(props) {
  const [friendsList, setFriendsList] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    async function fetchData() {
      try {
        const response = await getFriendList(props.token, props.userName);
        setFriendsList(response.payload);
        setLoading(false);
      } catch (error) {
        setError(error);
        setLoading(false);
      }
    }

    fetchData();
  }, [props.username]);

  return (
    <div className='app_module'>
      <Row>
        <Col lg={9}>
          <h3>Friends</h3>
        </Col>
        <Col lg={3}>
          <Button>...</Button>
        </Col>
      </Row>
      <div>
        {error}
      </div>
      {loading ? (
        <div>Loading...</div>
      ) : friendsList.length > 0 ? (
        <ul>
          {friendsList.map((friend) => (
            <li key={friend.id}>{friend.name}</li>
          ))}
        </ul>
      ) : (
        <div>No friends available.</div>
      )}
    </div>
  );
}
