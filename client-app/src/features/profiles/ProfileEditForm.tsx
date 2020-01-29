import React from "react";
import { observer } from "mobx-react-lite";
import { IProfile } from "../../app/models/profile";
import { combineValidators, isRequired } from "revalidate";
import { Form as FinalForm, Field } from "react-final-form";
import { Form, Button } from "semantic-ui-react";
import TextInput from "../../app/common/form/TextInput";
import TextAreaInput from "../../app/common/form/TextAreaInput";

const validate = combineValidators({
  displayName: isRequired("displayName")
});

interface IProps {
  updateProfile: (profile: Partial<IProfile>) => void;
  profile: IProfile;
  loading: boolean;
}

const ProfileEditForm: React.FC<IProps> = ({ updateProfile, profile, loading }) => {
  return (
    <FinalForm
      onSubmit={updateProfile}
      validate={validate}
      initialValues={profile!}
      render={({ handleSubmit, invalid, pristine }) => (
        <Form onSubmit={handleSubmit} error>
          <Field
            name="displayName"
            component={TextInput}
            placeholder="Display Name"
            value={profile!.displayName}
          />
          <Field
            name="bio"
            component={TextAreaInput}
            rows={3}
            placeholder="Bio"
            value={profile!.bio}
          />
          <Button
            loading={loading}
            floated="right"
            disabled={invalid || pristine}
            positive
            content="Update profile"
          />
        </Form>
      )}
    />
  );
};

export default observer(ProfileEditForm);
