import React, { useState, useEffect } from 'react';
import { Form } from 'react-bootstrap';
import BBVAPI, { query } from '../middleware/BBVAPI';
import { QUERY_TYPES } from '../middleware/types';

function SearchBar() {
  const [searchBody, setSearchBody] = useState('');
  const [items, setItems] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    return () => {
      setIsLoading(false);
    };
  }, []);

  const handleSearch = async (val) => {
    val = val.trim();
    setSearchBody(val);

    if (val.length > 2) {
      setIsLoading(true);
      try {
        const result = await query(QUERY_TYPES.ALL, val);
        setItems(result);
      } catch (err) {
        console.log(err);
      } finally {
        setIsLoading(false);
      }
    } else {
      setItems([]);
    }
  };

  const renderForm = () => {
    if(items.length > 0)
    {
        return (
            <Form className="AutoCompleteText">
            <Form.Control
              type="text"
              className="nav-search"
              placeholder="Search..."
              style={{ width: '100%' }}
              value={searchBody}
              onChange={(e) => handleSearch(e.target.value)}
            />
            {isLoading && <p>Loading...</p>}
            {items.length > 0 && (
              <ul>
                {items.map((item) => (
                  <li key={item.UserName}>
                    <a href={item.ProfileURL}>{item.UserName}</a>
                  </li>
                ))}
              </ul>
            )}
          </Form>
        )
    } else {
        return (
            <Form className="AutoCompleteText">
            <Form.Control
              type="text"
              className="nav-search"
              placeholder="Search..."
              style={{ width: '100%' }}
              value={searchBody}
              onChange={(e) => handleSearch(e.target.value)}
            />
          </Form>
        )
    }
  }

  return (
   renderForm()
  );
}

export default SearchBar;
