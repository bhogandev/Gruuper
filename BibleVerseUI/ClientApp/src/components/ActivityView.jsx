import React from 'react';
import { Button, Col, Row } from 'react-bootstrap';

export default function ActivityView() {
  return (
    <div className='app_module'>
      <Row>
        <Col lg={9}>
        <h3>Latest Activity</h3>
        </Col>
        <Col lg={3}>
        <Button>
          ...
        </Button>
        </Col>
        </Row>
      <div>
        none available.
      </div>
    </div>
  )
}
