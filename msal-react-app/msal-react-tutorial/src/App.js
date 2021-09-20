import React, { useState } from "react";
import { PageLayout } from "./components/PageLayout";
import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from "@azure/msal-react";
import { ProfileContent } from "./components/ProfileContent";

function App() {
  return (
      <PageLayout>
        <AuthenticatedTemplate>
          <ProfileContent />
        </AuthenticatedTemplate>
        <UnauthenticatedTemplate>
          <p>You are not signed in! Please sign in.</p>
        </UnauthenticatedTemplate>
      </PageLayout>
  );
}

export default App;