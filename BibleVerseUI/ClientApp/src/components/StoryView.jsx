import React from 'react'
import { Col, Row } from 'react-bootstrap'

export default function StoryView() {
  return (
    <div className='app_module'>
        <Row>
        <Col>
            One Story
        </Col>
        <Col>
            Two Story
        </Col>
        <Col>
            Three Story
        </Col>
        </Row>
    </div>
  )
}
