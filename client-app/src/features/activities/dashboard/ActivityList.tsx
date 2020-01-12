import React from "react";
import { Item, Button, Label, Segment } from "semantic-ui-react";
import { IActivity } from "../../../app/models/activity";

interface IProps {
  activities: IActivity[];
  selectActivity: (id: string) => void;
  deleteActivity: (id: string) => void;
}

export const ActivityList: React.FC<IProps> = ({
  activities,
  selectActivity,
  deleteActivity
}) => {
  return (
    <Segment clearing>
      <Item.Group divided>
        {activities.map(acitivity => (
          <Item key={acitivity.id}>
            <Item.Content>
              <Item.Header as="a">{acitivity.title}</Item.Header>
              <Item.Meta>{acitivity.date}</Item.Meta>
              <Item.Description>
                <div>{acitivity.description}</div>
                <div>
                  {acitivity.city}, {acitivity.venue}
                </div>
              </Item.Description>
              <Item.Extra>
                <Button
                  onClick={() => selectActivity(acitivity.id)}
                  floated="right"
                  content="View"
                  color="blue"
                />
                <Button
                  onClick={() => deleteActivity(acitivity.id)}
                  floated="right"
                  content="Delete"
                  color="red"
                />
                <Label basic content={acitivity.category} />
              </Item.Extra>
            </Item.Content>
          </Item>
        ))}
      </Item.Group>
    </Segment>
  );
};
