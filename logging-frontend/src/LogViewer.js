import React, { useState, useEffect } from 'react';
import dayjs from 'dayjs';
import { DemoContainer } from '@mui/x-date-pickers/internals/demo';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import { TextField, Grid, Typography, Accordion, AccordionSummary, AccordionDetails, List, ListItem, Select, MenuItem, FormControl, InputLabel } from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import ReactJson from 'react-json-view'
import 'dayjs/locale/ru';
import { getAggregatedEvents, getFilteredEvents, getFileEvents } from './backend.service.js';
import { BarChart } from '@mui/x-charts/BarChart';
import { DataGrid, GridToolbar } from '@mui/x-data-grid';
import { Menu, Paper, Button } from '@mui/material';
import { Tooltip, IconButton } from '@mui/material';
import HelpOutlineIcon from '@mui/icons-material/HelpOutline';

dayjs.locale('ru');

const columns = [
  { field: '_id', headerName: 'ID', width: 150 },
  { field: '@timestamp', headerName: 'Время', width: 220, valueGetter: ({ value }) => dayjs(value).format('YYYY.MM.DD HH:MM:SSS'), },
  {
    field: 'source',
    headerName: 'Событие',
    width: 1000,
    valueGetter: ({ value }) => JSON.stringify(value),
  }
];

const columnsAgg = [
  { field: 'key', headerName: 'Свойство', width: 150 },
  { field: 'value', headerName: 'Значение', width: 220, valueGetter: ({ row }) => {
    if(row.key !=  null)
      return `${row.value} шт.`
    else
      return `${row.value}`
    } 
  },
];

const InputWithTooltip = () => {
  return (
    <Tooltip title="Instructions or Help Text">
      <IconButton aria-label="Instructions">
        <HelpOutlineIcon />
      </IconButton>
    </Tooltip>
  );
};

const LogViewer = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [aggQuery, setAggQuery] = useState('');
  const [startDate, setStartDate] = useState(dayjs().subtract(10, 'day').startOf('day'));
  const [endDate, setEndDate] = useState(dayjs());
  const [logs, setLogs] = useState(null);
  const [agg, setAgg] = useState(null);
  const [aggFunc, setAggFunc] = useState(null);
  const [interval, setInterval] = useState(3600);
  const [dateRange, setDateRange] = useState(3600);
  const [selectedRow, setSelectedRow] = useState(null);
  const [exportType, setExportType] = useState("xlsx");

  const valueFormatter = (value) => {
    const date = new Date(value);
    const year = date.getFullYear();
    const month = (`0${date.getMonth() + 1}`).slice(-2);
    const day = (`0${date.getDate()}`).slice(-2);
    const hours = (`0${date.getHours()}`).slice(-2);
    const minutes = (`0${date.getMinutes()}`).slice(-2);
    const seconds = (`0${date.getSeconds()}`).slice(-2);
    return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
  }

  useEffect(() => {

    setDateRange(`${dayjs(startDate).format('YYYY.MM.DD HH:MM')} - ${dayjs(endDate).format('YYYY.MM.DD HH:MM')}`);
    fetchEventsData();

  }, [interval, startDate, endDate]);


  const fetchEventsData = async () => {

    try {
      const queryParams = {
        interval: interval,
        query: searchQuery.replace(/^\s+|\s+$/g, ''),
        filteredCount: 145,
        "@timestamp": {
          start: startDate,
          end: endDate,
        },
      };
      console.log(queryParams);
      const data = await getFilteredEvents(queryParams);
      setLogs(data.documents);
      setAgg(data.aggregations);
      setSelectedRow(null);
    } catch (error) {
      
    }
  };

  const fetchAggregationData = async () => {
    try {
      const queryParams = {
        "@timestamp": {
          start: startDate,
          end: endDate,
        },
        query: searchQuery.replace(/^\s+|\s+$/g, ''),
        aggregationQuery: aggQuery.replace(/^\s+|\s+$/g, ''),
      };

      const data = await getAggregatedEvents(queryParams);
      setAggFunc(data);
      setSelectedRow(null);
    } catch (error) {
      
    }
  };

  const downloadFile = async (fileType) => {
    try {

      const data = {
        query: searchQuery.replace(/^\s+|\s+$/g, ''),
        "@timestamp": {
          start: startDate,
          end: endDate,
        },
      };

      const response = await getFileEvents(data, fileType)
  
      const mimeType = fileType === 'csv' ? 'text/csv' : 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
      const blob = new Blob([response], { type: mimeType });
  
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `Выгрузка_событий_${dayjs().format('YYYY-MM-DD')}.${fileType}`);
      document.body.appendChild(link);
  
      link.click();
  
      window.URL.revokeObjectURL(url);
      document.body.removeChild(link);
    } catch (error) {
      console.error('Ошибка при загрузке файла:', error);
      // Обработка ошибок
    }
  };

  const handleDragStart = (event, logProp) => {
    event.dataTransfer.setData('text/plain', logProp);
  };

  const handleDragOver = (event) => {
    event.preventDefault();
  };

  function addIdToObjects(objectsList) {
    let idCounter = 1;
    const objectsWithIds = objectsList.map((object) => {
      if (object.id !== undefined) {
        return object; 
      }

      return { ...object, id: idCounter++ };
    });
    return objectsWithIds;
  }


  const handleRowClick = (params) => {
    console.log(params)
    if (params.row._id === selectedRow) {
      setSelectedRow(null);
    } else {
      setSelectedRow(params.row._id);
    }
  };

  const handlePropertyClick = (property) => {
    setSearchQuery((prevQuery) => prevQuery + ' ' + property);
  };

  const getObjectPropertiesAndTypesRecursive = (obj, parentKey = '') => {
    const properties = [];
  
    for (const key in obj) {
      if (Object.prototype.hasOwnProperty.call(obj, key)) {
        const prefixedKey = parentKey ? `${parentKey}.${key}` : key;
        const type = typeof obj[key];
        properties.push([prefixedKey, type]);
  
        if (type === 'object' && obj[key] !== null) {
          const nestedProperties = getObjectPropertiesAndTypesRecursive(obj[key], prefixedKey);
          properties.push(...nestedProperties);
        }
      }
    }
  
    return properties;
  };

  return (
    <div style={{ padding: '20px' }}>
      <Grid container spacing={2} alignItems="center">
      <Grid item xs={8}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} onDragOver={(e) => e.preventDefault()}>
            <TextField
              fullWidth
              label="Поисковый запрос"
              value={searchQuery}
              onChange={(newValue) => setSearchQuery(newValue.target.value)}
              onDrop={(event) => {
                event.preventDefault();
                const data = event.dataTransfer.getData('text/plain');
                setSearchQuery((prevQuery) => prevQuery + ' ' + data);
              }}
              InputProps={{
                endAdornment: <InputWithTooltip />,
              }}
            />
          </Grid>
          <Grid item xs={12}>
            <TextField
              fullWidth
              label="Агрегационный запрос"
              value={aggQuery}
              onChange={(newValue) => setAggQuery(newValue.target.value)}
              onDrop={(event) => {
                event.preventDefault();
                const data = event.dataTransfer.getData('text/plain');
                setAggQuery((prevQuery) => prevQuery + ' ' + data);
              }}
            />
          </Grid>       
        </Grid>
      </Grid>

      <Grid item xs={1}>
        <Grid container spacing={4}>
          <Grid item xs={12}>
          <Button variant="contained" 
          onClick={() => {
            if(aggQuery != '')
              fetchAggregationData()
            else
              fetchEventsData()
          }} 
          >Применить</Button>
          </Grid>
          <Grid item xs={12}>
          <Button variant="contained"
          onClick={() => {
            setAggQuery('')
            setSearchQuery('')
            fetchEventsData()
            setAggFunc(null)
          }}
          >Обновить</Button>
          </Grid>       
        </Grid>
      </Grid>
        
        <Grid item xs={3} marginBottom={2}>
            <LocalizationProvider dateAdapter={AdapterDayjs} adapterLocale="ru">
                <DemoContainer components={['DateTimePicker', 'DateTimePicker']}>
                    <DateTimePicker
                    ampm={false}
                        label="Начало периода"
                        value={startDate}
                        onChange={(newValue) => setStartDate(newValue)}
                    />
                    <DateTimePicker
                    ampm={false}
                        label="Конец периода"
                        value={endDate}
                        onChange={(newValue) => setEndDate(newValue)}
                    />
                </DemoContainer>
            </LocalizationProvider>
        </Grid>
      </Grid>

    <Grid container spacing={2}>
      {/* Первый блок (вертикально слева) */}
      <Grid item xs={4}>
        <Grid container spacing={12}>
          <Grid item xs={12}>
            {/* Список свойств log */}
            <Typography variant="subtitle1">Доступные поля</Typography>
            <List>
              {logs && getObjectPropertiesAndTypesRecursive(logs[0]).filter(el => el[1] != "object").map((property) => (
                <ListItem
                  key={property}
                  style={{
                    border: '1px dashed gray',
                    borderRadius: '5px',
                    marginBottom: '5px',
                    padding: '5px',
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                  }}
                  //onClick={() => handlePropertyClick(property[0])}
                  draggable
                  onDragStart={(event) => handleDragStart(event, property[0])}
                  onDragOver={handleDragOver}
                  onMouseEnter={(event) => {
                    event.currentTarget.style.backgroundColor = 'lightgrey';
                  }}
                  onMouseLeave={(event) => {
                    event.currentTarget.style.backgroundColor = 'inherit';
                  }}
                >
                  {`${property[0]}: ${property[1]}`}
                  <span>➕</span>
                </ListItem>
              ))}
            </List>      
          </Grid>
          <Grid item xs={12}>
            { selectedRow && <ReactJson src={logs.find(l => l._id == selectedRow)} /> }
          </Grid>
        </Grid>
      </Grid>

      {/* Второй блок (вертикально справа от первого) */}
      {aggFunc && 
      <Grid item xs={8} sx={{ marginTop: '30px' }}>
            <DataGrid
            rows={addIdToObjects(aggFunc.result)}
            columns={columnsAgg}
            onRowClick={handleRowClick}
            initialState={{
              pagination: { paginationModel: { pageSize: 10 }},
            }}
            pageSizeOptions={{label: "" }}
             />        
      </Grid>
      }
      { !aggFunc &&
        <Grid item xs={8} sx={{ marginTop: '30px' }}>

          <Grid container spacing={2}>

          <Grid item xs={4} width="100%" sx={{ marginTop: '17px' }}>
            <Typography variant="body1" fontWeight="bold" gutterBottom>Всего: {agg && agg.reduce((accumulator, currentValue) => {
                return accumulator + currentValue.count;
              }, 0)}
            </Typography>
          </Grid>

          <Grid item xs={4} width="100%"  sx={{ marginTop: '17px' }}>
            <Typography variant="body1" fontWeight="300" color="textSecondary" fontSize={16} gutterBottom>{dateRange}</Typography>
          </Grid>  

          <Grid item xs={4} width="100%">
            <Grid container spacing={2} alignItems="center">
              <Grid item>
                <InputLabel id="interval-label">Выберите интервал</InputLabel>
              </Grid>
              <Grid item>
                <FormControl>
                  <Select
                    labelId="interval-label"
                    id="interval-select"
                    value={interval}
                    label="Выберите интервал"
                    onChange={(newValue) => setInterval(newValue.target.value)}
                  >
                    <MenuItem value={300}>5 мин.</MenuItem>
                    <MenuItem value={900}>15 мин.</MenuItem>
                    <MenuItem value={1800}>30 мин.</MenuItem>
                    <MenuItem value={3600}>1 ч.</MenuItem>
                    <MenuItem value={86400}>24 ч.</MenuItem>
                    {/* Добавьте другие интервалы при необходимости */}
                  </Select>
                </FormControl>
              </Grid>
            </Grid>
          </Grid>
            
          </Grid>

          <Grid container height="100%">
            {/* Второй блок (на всю ширину) */}
            <Grid item xs={12} width="100%">
              { (agg && agg.length !=0 ) &&
                <BarChart
                  dataset={agg}
                  xAxis={[{ scaleType: 'band', dataKey: '@timestamp', valueFormatter }]}
                  series={[{ dataKey: 'count' }]}
                  layout="vertical"
                  height={320}
              />
              }
            </Grid>
            <Grid item xs={12} width="100%" marginRight={3}>
            <Grid container spacing={2} alignItems="center" direction="row" justifyContent="flex-end">
              <Grid item>
                <Button variant="contained"
                onClick={() => downloadFile(exportType)}
                >Экспорт</Button>
              </Grid>
              <Grid item>
                  <Select
                    value={exportType}
                    onChange={(newValue) => setExportType(newValue.target.value)}
                  >
                    <MenuItem value="xlsx">xlsx</MenuItem>
                    <MenuItem value="csv">csv</MenuItem>
                  </Select>
              </Grid>
            </Grid>            
            </Grid>
            <Grid item xs={12} width="100%">
              {(logs && columns) && 
              <div>

              <DataGrid
              rows={logs}
              columns={columns}
              getRowId={(row) => row._id}
              onRowClick={handleRowClick}
              initialState={{
                pagination: { paginationModel: { pageSize: 10 }},
              }}
              pageSizeOptions={{label: "" }}
              />
                          
              </div>
              }
            </Grid>
          </Grid>

        </Grid>
      }

    </Grid>

    </div>
  );
};


export default LogViewer;
