import React from 'react'
import { Button, Col, Row } from 'react-bootstrap'

export default function GroupView() {
  return (
    <div className='app_module'>
      <Row>
        <Col lg={9}>
        <h3>GroupView</h3>
        </Col>
        <Col lg={3}>
        <Button>
          ...
        </Button>
        </Col>
      </Row>

      <div>
        no groups available.
      </div>
    </div>
  )
}
