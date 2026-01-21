const parentLoginInteractionRoute = async (routeOptions) => {
  const { provider, azureParentIntegration, generators, instance, ssHandler, req, res, next } =
    routeOptions;
  try {
    const client = await azureParentIntegration;
    const code_verifier = generators.codeVerifier();
    const code_challenge = generators.codeChallenge(code_verifier);

    const interactionDetails = await provider.interactionDetails(req, res);
    const { uid, adapter } = interactionDetails;
    await adapter.upsert(
      `oidc-azure-challenge:${uid}`,
      code_verifier,
      +process.env.AZURE_CHALLENGE_EXPIRATION,
    );

    const cookieOptions = instance(provider).configuration('cookies.short');
    const ctx = provider.app.createContext(req, res);
    ssHandler.set(ctx.cookies, provider.cookieName('interaction'), uid, {
      ...cookieOptions,
      httpOnly: true,
      sameSite: false, // needed as azure does additional requests behind the scenes when client has multiple accounts
    });

    const authUrl = client.authorizationUrl({
      scope: 'openid profile email',
      code_challenge,
      code_challenge_method: 'S256',
      prompt: 'login',
    });
    return res.redirect(authUrl);
  } catch (err) {
    next(err);
  }
};

module.exports = parentLoginInteractionRoute;
