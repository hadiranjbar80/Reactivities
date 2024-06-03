import { Button, Form, Segment } from "semantic-ui-react";
import { Activity } from "../../../app/models/activity";
import { ChangeEvent, useState } from "react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";


export default observer(function ActivityForm() {
  const {activityStore} = useStore();
  const {selectedActivity, closeForm, createActivity, updateActivity, loading} = activityStore

  const initialState = selectedActivity ?? {
    id: '',
    title: '',
    description: '',
    date: '',
    category: '',
    city: '',
    venue: ''
  }

  const [activity, setActivity] = useState(initialState)

  const handleSubmit = () =>{
    activity.id ? updateActivity(activity) : createActivity(activity);

  }

  const handleInputChange = (event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const {name, value} = event.target;
    setActivity({...activity, [name]: value})
  }

  return (
    <Segment clearing>
        <Form onSubmit={handleSubmit}>
            <Form.Input placeholder='Title' value={activity.title} onChange={handleInputChange} name='title' />
            <Form.TextArea placeholder='Description' value={activity.description} onChange={handleInputChange} name='description' />
            <Form.Input placeholder='Category' value={activity.category} onChange={handleInputChange} name='category' />
            <Form.Input type="date" placeholder='Date' value={activity.date} onChange={handleInputChange} name='date' />
            <Form.Input placeholder='City' value={activity.city} onChange={handleInputChange} name='city' />
            <Form.Input placeholder='Venue' value={activity.venue} onChange={handleInputChange} name='venue' />
            <Button loading={loading} floated="right" positive type="submit" content="Submit" />
            <Button onClick={closeForm} floated="right" type="button" content="Candel" />
        </Form>
    </Segment>
  )
})
